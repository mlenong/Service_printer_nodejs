require('dotenv').config();
const express = require('express');
const ptp = require('pdf-to-printer');
const fs = require('fs');
const multer = require('multer');
const { execFile } = require('child_process');
const path = require('path');
const os = require('os');

// Resolve the directory where the executable is located
// process.execPath is the path to the executable (e.g., service-backend.exe)
// path.dirname gives us the folder containing the exe.
const baseDir = path.dirname(process.execPath);

// Use user's temp directory for file uploads to avoid permission issues in Program Files
const filesDir = path.join(os.tmpdir(), "service-print-files");
const sumatraPath = path.join(baseDir, "SumatraPDF.exe");

// Ensure temp 'files' directory exists
if (!fs.existsSync(filesDir)) {
    try {
        fs.mkdirSync(filesDir);
    } catch (err) {
        console.error("Failed to create temp directory", err);
    }
}

const app = express();
const port = process.env.port || 3000;

const upload = multer({
    storage: multer.diskStorage({
        destination: filesDir,
        filename: (req, file, cb) => {
            if (file.mimetype === 'application/pdf') {
                cb(null, Date.now() + "-" + file.originalname)
            } else {
                cb(new Error('Only PDF file is allowed'), false);
            }
        }
    }),
})

const auth = function (req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    next()
}

const asyncHandler = (fn) => (req, res, next) => {
    Promise.resolve(fn(req, res, next)).catch(next);
};

app.get('/', asyncHandler(async (req, res) => {
    let printers = await ptp.getPrinters()
    return res.status(200).json({
        data: printers,
        message: "success"
    });
}))

app.get('/ping', asyncHandler(async (req, res) => {
    return res.status(200).json({
        message: "service print ready !!!"
    });
}))

app.use(auth)

app.post("/print", upload.single('file'), asyncHandler(async (req, res) => {
    if (!req.file) {
        return res.status(404).json({
            message: "file is required"
        })
    }

    const args = ["-print-to-default", req.file.path];

    if (req.body.printer_name) {
        // print(req.file.path, { printer: ... }) replacement
        // SumatraPDF -print-to "printer" -print-settings "1x" file.pdf
        // If copies needed: -print-settings "Nx" where N is copies
        const copies = req.body.copy || 1;
        args[0] = "-print-to";
        args[1] = req.body.printer_name;
        args.push("-print-settings", `${copies}x`);
        args.push(req.file.path);
    }

    // Execute SumatraPDF
    // We wrap it in a promise
    await new Promise((resolve, reject) => {
        execFile(sumatraPath, args, (error, stdout, stderr) => {
            if (error) {
                // If sumatra returns error code, we might want to log it but maybe resolve if it printed?
                // Sumatra usually returns 0 on success.
                reject(error);
            } else {
                resolve(stdout);
            }
        });
    });

    try {
        fs.unlinkSync(req.file.path);
    } catch (e) {
        console.error("Failed to delete temp file", e);
    }

    return res.status(200).json({
        message: "success"
    });
}));

app.use((err, req, res, next) => {
    if (req.file) {
        try {
            fs.unlinkSync(req.file.path);
        } catch (e) { }
    }

    return res.status(500).json({
        message: err.message
    });
});

app.listen(port, () => {
    console.log(`Service running on port ${port}`)
});
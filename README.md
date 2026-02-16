# Service Print Professional

Aplikasi untuk menangani pencetakan PDF di latar belakang melalui REST API, dibungkus dalam aplikasi Windows System Tray.

## 📋 Prasyarat

Untuk membangun (build) project ini dari source code, Anda membutuhkan:

1.  **Node.js** (Versi 18+ disarankan)
    - Pastikan `node` dan `npm` bisa dipanggil di command prompt.
2.  **Inno Setup 6** (untuk membuat file installer)
    - Pastikan `ISCC.exe` ada di PATH atau di folder default `C:\Program Files (x86)\Inno Setup 6\`
3.  **Microsoft .NET Framework** (Bawaan Windows)
    - Digunakan untuk compiler C# `csc.exe`.

## 📂 Struktur Project

- **src/**
    - `index.js`: Service backend utama berbasis Node.js (Express API).
    - `ServicePrintTray.cs`: Source code C# untuk aplikasi System Tray.
- **service-backend.exe**: Aplikasi Node.js yang sudah di-compile (hasil generate).
- **ServicePrintTray.exe**: Aplikasi C# yang sudah di-compile (hasil generate).
- **SumatraPDF.exe**: Engine pencetak PDF (sudah dibundel).
- **printer.ico**: Icon aplikasi.
- **setup.iss**: Script konfigurasi Inno Setup.
- **build_installer.bat**: **Master Script** (Menjalankan semua proses build).

## 🚀 Cara Build (Membuat Aplikasi)

Kami telah menyediakan script otomatis untuk mempermudah proses build.

### 1. Install Dependensi
Jalankan perintah ini di terminal (cmd/powershell) pada folder project:
```bash
npm install
```

### 2. Build Lengkap (Rekomendasi)
Jalankan script utama untuk mem-build backend, frontend, dan installer sekaligus:
```cmd
build_installer.bat
```
Hasilnya akan jadi satu file `ServicePrintInstaller.exe` di folder project.

### 3. Build Modular (Manual)
Jika ingin mem-build per komponen secara terpisah:

**A. Build Backend Node.js**
Mengubah `src/index.js` menjadi `service-backend.exe`.
```cmd
build_backend.bat
```

**B. Build Wrapper C#**
Mengubah `src/ServicePrintTray.cs` menjadi `ServicePrintTray.exe`.
```cmd
compile_wrapper.bat
```

**C. Buat Installer**
Membungkus semua file menjadi `ServicePrintInstaller.exe`.
```cmd
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" setup.iss
```

## 🛠️ Pengembangan (Development)

### Menjalankan Backend Secara Lokal
Anda bisa menjalankan backend langsung pakai Node.js untuk debugging:
```bash
node src/index.js
```
API akan jalan di `http://localhost:3000`.

### Fitur Utama
- **Auto-Start**: Wrapper C# menambahkan registry key ke `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`.
- **Single Instance**: Menggunakan `Mutex` agar aplikasi tidak bisa dijalankan ganda (double).
- **Fix Permission**: Menggunakan folder `%TEMP%` untuk upload file PDF agar tidak kena masalah "Access Denied" di Program Files.
- **Robust Pathing**: Menggunakan `process.execPath` untuk mencari lokasi resource relatif terhadap file exe.

## 📦 Distribusi
Script `collect_dist.bat` akan mengumpulkan semua file yang diperlukan untuk rilis versi Portable ke dalam folder `Service Print 3000`.
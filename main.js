"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var elemon = require("elemon");
var fs = require("fs-jetpack");
var path = require("path");
var url = require("url");
var win, serve;
var args = process.argv.slice(1);
serve = args.some(function (val) { return val === '--serve'; });
function createWindow() {
    var electronScreen = electron_1.screen;
    var size = electronScreen.getPrimaryDisplay().workAreaSize;
    // Create the browser window.
    win = new electron_1.BrowserWindow({
        x: 0,
        y: 0,
        width: size.width,
        height: size.height
    });
    if (serve) {
        require('electron-reload')(__dirname, {
            electron: require(__dirname + "/node_modules/electron")
        });
        win.loadURL('http://localhost:4200');
    }
    else {
        win.loadURL(url.format({
            pathname: path.join(__dirname, 'dist/index.html'),
            protocol: 'file:',
            slashes: true
        }));
    }
    win.webContents.openDevTools();
    // Emitted when the window is closed.
    win.on('closed', function () {
        // Dereference the window object, usually you would store window
        // in an array if your app supports multi windows, this is the time
        // when you should delete the corresponding element.
        win = null;
    });
}
try {
    electron_1.ipcMain.on('load-dir', function (event, arg) {
        electron_1.dialog.showOpenDialog({
            title: "Select a folder",
            properties: ["openDirectory"]
        }, function (folderPaths) {
            // folderPaths is an array that contains all the selected paths
            if (folderPaths === undefined) {
                console.log("No destination folder selected");
                event.returnValue = "";
            }
            else {
                event.returnValue = folderPaths;
            }
        });
    });
    electron_1.ipcMain.on('load-file', function (event, filePath) {
        var fileData = fs.read(filePath);
        event.returnValue = fileData;
    });
    electron_1.ipcMain.on('load-json', function (event, filePath) {
        console.log('EVENT:', event);
        console.log('FILE PATH:', filePath);
        var fileData = fs.read(filePath, "json");
        event.returnValue = fileData;
    });
    // This method will be called when Electron has finished
    // initialization and is ready to create browser windows.
    // Some APIs can only be used after this event occurs.
    electron_1.app.on('ready', createWindow);
    // Quit when all windows are closed.
    electron_1.app.on('window-all-closed', function () {
        // On OS X it is common for applications and their menu bar
        // to stay active until the user quits explicitly with Cmd + Q
        if (process.platform !== 'darwin') {
            electron_1.app.quit();
        }
    });
    electron_1.app.on('activate', function () {
        // On OS X it's common to re-create a window in the app when the
        // dock icon is clicked and there are no other windows open.
        if (win === null) {
            createWindow();
            elemon({
                app: electron_1.app,
                mainFile: 'main.js',
                bws: [
                    { bw: win, res: ['index.html', '', 'styles.css'] }
                ]
            });
        }
    });
}
catch (e) {
    // Catch Error
    // throw e;
}

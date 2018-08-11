"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var fs = require("fs-jetpack");
var IPCFileListener = /** @class */ (function () {
    function IPCFileListener() {
    }
    IPCFileListener.setUpIPC = function (win) {
        electron_1.ipcMain.on('launch-directory', function (event) {
            event.returnValue = electron_1.app.getAppPath();
        });
        electron_1.ipcMain.on('pick-directory', function (event) {
            var dir = electron_1.dialog.showOpenDialog(win, {
                properties: ['openDirectory']
            });
            event.returnValue = dir;
        });
        electron_1.ipcMain.on('create-directory', function (event, directoryPath) {
            fs.dir(directoryPath);
            event.returnValue = '';
        });
        electron_1.ipcMain.on('exists-item', function (event, itemPath) {
            var data = fs.exists(itemPath);
            event.returnValue = data;
        });
        electron_1.ipcMain.on('load-file', function (event, filePath) {
            var data = fs.read(filePath);
            event.returnValue = data;
        });
        electron_1.ipcMain.on('find-files', function (event, filePath, matching) {
            var data = fs.find(filePath, matching);
            event.returnValue = data;
        });
        electron_1.ipcMain.on('load-file-async', function (event, filePath) {
            var test = '';
        });
        electron_1.ipcMain.on('load-json-file', function (event, filePath) {
            var data = fs.read(filePath, 'utf8');
            data = JSON.parse(data);
            event.returnValue = data;
        });
        electron_1.ipcMain.on('save-file', function (event, filePath, fileData) {
            // convert non-string to json string otherwise leave alone
            fileData =
                typeof fileData !== 'string' ? JSON.stringify(fileData) : fileData;
            fs.write(filePath, fileData);
            event.returnValue = '';
        });
        electron_1.ipcMain.on('delete-item', function (event, itemPath) {
            // convert non-string to json string otherwise leave alone
            fs.remove(itemPath);
            event.returnValue = '';
        });
    };
    return IPCFileListener;
}());
exports.IPCFileListener = IPCFileListener;

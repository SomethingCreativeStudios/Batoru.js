import { app, BrowserWindow, screen, dialog, ipcMain } from 'electron';
import * as fs from 'fs-jetpack';

export class IPCFileListener {
    constructor() {
    }

    public static setUpIPC(win: any) {
        ipcMain.on('launch-directory', async function (event) {
            event.returnValue = app.getAppPath();
        });

        ipcMain.on('pick-directory', async function (event) {
            const dir = await dialog.showOpenDialog(win, {
                properties: ['openDirectory'],
            });
            event.returnValue = dir;
        });

        ipcMain.on('create-directory', async function (event, directoryPath) {
            fs.dir(directoryPath);
            event.returnValue = '';
        });

        ipcMain.on('exists-item', async function (event, itemPath) {
            const data = fs.exists(itemPath);
            event.returnValue = data;
        });

        ipcMain.on('load-file', async function (event, filePath) {
            const data = fs.read(filePath);
            event.returnValue = data;
        });

        ipcMain.on('find-files', async function (event, filePath, matching) {
            const data = fs.find(filePath, matching);
            event.returnValue = data;
        });

        ipcMain.on('load-file-async', async function (event, filePath) {
            const data = fs.readAsync(filePath).then(result => {
                event.returnValue = result;
            }).catch(error => {
                event.returnValue = error;
            });
        });

        ipcMain.on('load-json-file', async function (event, filePath) {
            let data = fs.read(filePath, 'utf8');
            data = JSON.parse(data);
            event.returnValue = data;
        });

        ipcMain.on('save-file', async function (event, filePath, fileData) {
            // convert non-string to json string otherwise leave alone
            fileData = typeof fileData !== 'string' ? JSON.stringify(fileData) : fileData;
            fs.write(filePath, fileData);
            event.returnValue = '';
        });

        ipcMain.on('delete-item', async function (event, itemPath) {
            // convert non-string to json string otherwise leave alone
            fs.remove(itemPath);
            event.returnValue = '';
        });
    }
}
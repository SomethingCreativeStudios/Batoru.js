import { app, dialog, ipcMain } from 'electron';
import * as fs from 'fs-jetpack';

export class IPCFileListener {
  public static setUpIPC(win: any) {
    ipcMain.on('launch-directory', event => {
      event.returnValue = app.getAppPath();
    });

    ipcMain.on('pick-directory', event => {
      const dir = dialog.showOpenDialog(win, {
        properties: ['openDirectory'],
      });
      event.returnValue = dir;
    });

    ipcMain.on('create-directory', (event, directoryPath) => {
      fs.dir(directoryPath);
      event.returnValue = '';
    });

    ipcMain.on('exists-item', (event, itemPath) => {
      const data = fs.exists(itemPath);
      event.returnValue = data;
    });

    ipcMain.on('load-file', (event, filePath) => {
      const data = fs.read(filePath);
      event.returnValue = data;
    });

    ipcMain.on('find-files', (event, filePath, matching) => {
      const data = fs.find(filePath, matching);
      event.returnValue = data;
    });

    ipcMain.on('load-file-async', (event, filePath) => {
      const test = '';
    });

    ipcMain.on('load-json-file', (event, filePath) => {
      let data = fs.read(filePath, 'utf8');
      data = JSON.parse(data);
      event.returnValue = data;
    });

    ipcMain.on('save-file', (event, filePath, fileData) => {
      // convert non-string to json string otherwise leave alone
      fileData =
        typeof fileData !== 'string' ? JSON.stringify(fileData) : fileData;
      fs.write(filePath, fileData);
      event.returnValue = '';
    });

    ipcMain.on('delete-item', (event, itemPath) => {
      // convert non-string to json string otherwise leave alone
      fs.remove(itemPath);
      event.returnValue = '';
    });
  }
}

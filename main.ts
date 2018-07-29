import { app, BrowserWindow, screen, dialog, ipcMain } from 'electron';
import * as fs from "fs-jetpack";
import * as path from 'path';
import * as url from 'url';

import { ConnectionOptions, } from 'typeorm';

let win, serve;
const args = process.argv.slice(1);
serve = args.some(val => val === '--serve');

function createWindow() {

  const electronScreen = screen;
  const size = electronScreen.getPrimaryDisplay().workAreaSize;

  // Create the browser window.
  win = new BrowserWindow({
    x: 0,
    y: 0,
    width: size.width,
    height: size.height
  });

  if (serve) {
    require('electron-reload')(__dirname, {
      electron: require(`${__dirname}/node_modules/electron`)
    });
    win.loadURL('http://localhost:4200');
  } else {
    win.loadURL(url.format({
      pathname: path.join(__dirname, 'dist/index.html'),
      protocol: 'file:',
      slashes: true
    }));
  }

  win.webContents.openDevTools();

  // Emitted when the window is closed.
  win.on('closed', () => {
    // Dereference the window object, usually you would store window
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    win = null;
  });
}

try {

  ipcMain.on('load-dir', (event, arg) => {
    dialog.showOpenDialog({
      title: "Select a folder",
      properties: ["openDirectory"]
    }, (folderPaths) => {
      // folderPaths is an array that contains all the selected paths
      if (folderPaths === undefined) {
        console.log("No destination folder selected");
        event.returnValue = "";
      } else {
        event.returnValue = folderPaths;
      }
    });
  })

  ipcMain.on('load-file', (event, filePath) => {
    var fileData = fs.read(filePath);
    event.returnValue = fileData;
  });

  ipcMain.on('load-json', (event, filePath) => {
    console.log('EVENT:', event);
    console.log('FILE PATH:', filePath)
    var fileData = fs.read(filePath, "json");
    event.returnValue = fileData;
  });

  // This method will be called when Electron has finished
  // initialization and is ready to create browser windows.
  // Some APIs can only be used after this event occurs.
  app.on('ready', createWindow);

  // Quit when all windows are closed.
  app.on('window-all-closed', () => {
    // On OS X it is common for applications and their menu bar
    // to stay active until the user quits explicitly with Cmd + Q
    if (process.platform !== 'darwin') {
      app.quit();
    }
  });

  app.on('activate', () => {
    // On OS X it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (win === null) {
      createWindow();
    }
  });

} catch (e) {
  // Catch Error
  // throw e;
}

import { Injectable } from '@angular/core';
import { ElectronService } from '../../providers/electron.service';
import { resolve } from 'url';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  constructor(private _electronService: ElectronService) { }

  /**
   * Open default directory dialog.
   * @returns {string} full path to directory
   */
  public pickDirectory(): string {
    let directoryPath = '';
    if (this._electronService.isElectron) {
      directoryPath = <string>this._electronService.ipcRenderer.sendSync('pick-directory');
    }
    return directoryPath;
  }

  /**
   * Create a new directory
   * @param {string} directoryPath full path to directory to be created
   * @returns {string} directoryPath
   */
  public createDirectory(directoryPath: string): string {
    if (this._electronService.isElectron) {
      this._electronService.ipcRenderer.sendSync('create-directory', directoryPath);
    }
    return directoryPath;
  }

  public exists(path: string): boolean {
    let doesExists = false;
    if (this._electronService.isElectron) {
      doesExists = this._electronService.ipcRenderer.sendSync('exists-item', path);
    }
    return doesExists;
  }

  /**
   * Loads file and returns it's content
   * @param filePath path to file
   * @returns {any} contents of file
   */
  public loadFile(filePath: string): any {
    let fileData = null;
    if (this._electronService.isElectron) {
      fileData = this._electronService.ipcRenderer.sendSync('load-file', filePath);
    }
    return fileData;
  }

  /**
   * Loads json file into an object
   * @param filePath path to file
   * @returns {any} json object
   */
  public loadJSONFile(filePath: string): any {
    let fileData = null;
    if (this._electronService.isElectron) {
      fileData = this._electronService.ipcRenderer.sendSync('load-json-file', filePath);
    }
    return fileData;
  }

  /**
   * Removed a file or directory based on item Path
   * @param itemPath path to item
   */
  public removeItem(itemPath: string): any {
    if (this._electronService.isElectron) {
      this._electronService.ipcRenderer.sendSync('delete-item', itemPath);
    }
  }


  /**
   * Writes fileData to file. This will overwrite the file, if it exists.
   * @param filePath
   * @param fileData
   */
  public writeFile(filePath: string, fileData: any) {
    if (this._electronService.isElectron) {
      this._electronService.ipcRenderer.sendSync('save-file', filePath, fileData);
    }
  }


  /**
   * Writes fileData to file. This will overwrite the file, if it exists.
   * @param filePath
   * @param fileData
   */
  public writeFileAsync(filePath: string, fileData: any) {
    if (this._electronService.isElectron) {
      this._electronService.ipcRenderer.sendSync('save-file-async', filePath, fileData);
    }
  }

  /**
 * Reads a file async.  Returns a promise that has the file data.
 * @param filePath
 * @param fileData
 */
  public loadFileAsync(filePath: string) {
    return new Promise((resolve, reject) => {
      let fileData = {};
      if (this._electronService.isElectron) {
        fileData = this._electronService.ipcRenderer.sendSync('load-file-async', filePath);
      }

      resolve(fileData);
    });
  }
}
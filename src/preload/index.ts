import { contextBridge, ipcRenderer, IpcRendererEvent } from 'electron';
import { electronAPI } from '@electron-toolkit/preload';
import { API } from './api';

// Custom APIs for renderer
const api: API = {
  subscribe<T>(eventType: string, observer: (data: T) => void) {
    const listener = (_: IpcRendererEvent, data: T) => {
      observer(data);
    };
    ipcRenderer.on(eventType, listener);
    return {
      unsubscribe() {
        ipcRenderer.removeListener(eventType, listener);
      },
    };
  },
  getMicrophoneList: function () {
    return ipcRenderer.invoke('microphones-list');
  },
};

// Use `contextBridge` APIs to expose Electron APIs to
// renderer only if context isolation is enabled, otherwise
// just add to the DOM global.
if (process.contextIsolated) {
  try {
    contextBridge.exposeInMainWorld('electron', electronAPI);
    contextBridge.exposeInMainWorld('api', api);
  } catch (error) {
    console.error(error);
  }
} else {
  // @ts-ignore (define in dts)
  window.electron = electronAPI;
  // @ts-ignore (define in dts)
  window.api = api;
}

import { ElectronAPI } from '@electron-toolkit/preload';

export interface Api {
  subscribe: (observer: (event: IpcRendererEvent, message: string) => void) => void;
  unsubscribe: (observer: (event: IpcRendererEvent, message: string) => void) => void;
}

declare global {
  interface Window {
    electron: ElectronAPI;
    api: Api;
  }
}

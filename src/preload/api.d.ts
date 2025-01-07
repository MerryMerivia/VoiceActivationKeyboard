export interface Subscription {
  unsubscribe: () => void;
}

export type Observer<T> = (data: T) => void;

export interface WaveInCapabilities {
  channels: number;
  manufacturerGuid: string;
  nameGuid: string;
  productGuid: string;
  productName: string;
}

export interface API {
  subscribe(eventType: 'voice', observer: Observer<number>): Subscription;
  getMicrophoneList(): Promise<WaveInCapabilities[]>;
}

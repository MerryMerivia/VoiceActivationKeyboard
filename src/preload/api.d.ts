export interface Subscription {
  unsubscribe: () => void;
}

export type Observer<T> = (data: T) => void;

export interface API {
  subscribe(eventType: 'voice', observer: Observer<string>): Subscription;
}

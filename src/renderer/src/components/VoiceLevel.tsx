import { IpcRendererEvent } from 'electron';
import { useEffect, useState } from 'react';

function VoiceLevel() {
  const [text, setText] = useState<string>('');

  useEffect(() => {
    const listener: (event: IpcRendererEvent, message: string) => void = (event, message) => {
      setText(message);
    };
    window.api.subscribe(listener);
    return (): void => {
      window.api.unsubscribe(listener);
    };
  }, []);

  return <div>{text}</div>;
}

export default VoiceLevel;

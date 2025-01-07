import { useEffect, useState } from 'react';

function VoiceLevel() {
  const [text, setText] = useState<string>('');

  useEffect(() => {
    const subscription = window.api.subscribe('voice', (message) => setText(message));
    return () => {
      subscription.unsubscribe();
    };
  }, []);

  return <div>{text}</div>;
}

export default VoiceLevel;

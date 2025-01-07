import { useEffect, useState } from 'react';

function VoiceLevel() {
  const [fraction, setFraction] = useState<number>(0);

  useEffect(() => {
    const subscription = window.api.subscribe('voice', (fraction) => setFraction(fraction));
    return () => {
      subscription.unsubscribe();
    };
  }, []);

  return <div>{fraction}</div>;
}

export default VoiceLevel;

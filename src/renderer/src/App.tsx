import { useEffect, useState } from 'react';
import VoiceLevel from './components/VoiceLevel';
import { WaveInCapabilities } from 'src/preload/api';

function App(): JSX.Element {
  const [microphones, setMicrophones] = useState<WaveInCapabilities[]>([]);

  useEffect(() => {
    window.api.getMicrophoneList().then(
      (response) => {
        setMicrophones(response);
        console.log('microphones', response);
      },
      (err) => {
        setMicrophones([]);
        console.error('error', err);
      },
    );
  }, []);

  return (
    <>
      {microphones.map((micro) => (
        <div key={micro.productGuid}>{micro.productName}</div>
      ))}
      <VoiceLevel />
    </>
  );
}

export default App;

import { useState } from 'react';
import { UnityTest } from './unity-test';
// import { EthosConnectProvider } from 'ethos-connect';
const fcl = require('@blocto/fcl');
// Create an AptosClient to interact with devnet.

// fcl
//   .config()
//   .put('accessNode.api', 'https://rest-testnet.onflow.org')
//   .put('discovery.wallet.method', 'POP/RPC')
//   .put('challenge.handshake', 'https://flow-wallet-testnet.blocto.app/authn');

export default function App(props: any) {
  const [isMounted, setIsMounted] = useState(false);
  function handleClickToggleMount() {
    setIsMounted(!isMounted);
  }

  return (
    <div>
      {/* <button onClick={handleClickToggleMount}>
        {isMounted ? 'Unmount' : 'Mount'} Unity App
      </button>
      {isMounted && <UnityTest />} */}
      <UnityTest />
    </div>
  );
}

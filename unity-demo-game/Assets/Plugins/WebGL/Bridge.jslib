mergeInto(LibraryManager.library, {
  Web3Connect: function (invocationId, reqObj) {
    try {
      window.dispatchReactUnityEvent(
        'Web3Connect',
        UTF8ToString(invocationId),
        UTF8ToString(reqObj)
      );
    } catch (e) {
      console.warn('Failed to dispatch Web3Connect event...');
    }
  },
  FetchWalletBalance: function (invocationId, address) {
    try {
      window.dispatchReactUnityEvent(
        'FetchWalletBalance',
        UTF8ToString(invocationId),
        UTF8ToString(address)
      );
    } catch (e) {
      console.warn('Failed to dispatch FetchWalletBalance event...');
    }
  },
  RequestChangeNetwork: function (networkName) {
    try {
      window.dispatchReactUnityEvent(
        'RequestChangeNetwork',
        UTF8ToString(networkName)
      );
    } catch (e) {
      console.warn('Failed to dispatch RequestChangeNetwork event...');
    }
  },
  SendFlowQuery: function (invocationId, reqObj) {
    try {
      window.dispatchReactUnityEvent(
        'SendFlowQuery',
        UTF8ToString(invocationId),
        UTF8ToString(reqObj)
      );
    } catch (e) {
      console.warn('Failed to dispatch FlowQuery event...');
    }
  },
  SendEthereumTransaction: function (invocationId, reqObj) {
    try {
      window.dispatchReactUnityEvent(
        'SendEthereumTransaction',
        UTF8ToString(invocationId),
        UTF8ToString(reqObj)
      );
    } catch (e) {
      console.warn('Failed to dispatch FlowQuery event...');
    }
  },
  Web3SignMessage: function (invocationId, reqObj) {
    try {
      window.dispatchReactUnityEvent(
        'Web3SignMessage',
        UTF8ToString(invocationId),
        UTF8ToString(reqObj)
      );
    } catch (e) {
      console.warn('Failed to dispatch Web3SignMessage event...');
    }
  },
SendAptosTransaction: function (invocationId, reqObj) {
    try {
     window.dispatchReactUnityEvent(
        'SendAptosTransaction',
        UTF8ToString(invocationId),
        UTF8ToString(reqObj)
      );
    } catch(e) {
      console.warn('Failed to dispatch SendAptosTransaction event...')
    }
},
SendSuiTransaction: function (invocationId, reqObj) {
    try {
     window.dispatchReactUnityEvent(
        'SendSuiTransaction',
        UTF8ToString(invocationId),
        UTF8ToString(reqObj)
      );
    } catch(e) {
      console.warn('Failed to dispatch SendSuiTransaction event...')
    }
}
});

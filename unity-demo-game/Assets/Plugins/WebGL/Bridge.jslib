mergeInto(LibraryManager.library, {
  JSSDKInteraction: function (invocationId, methodName, reqJSON) {
    try {
      window.dispatchReactUnityEvent(
        'JSSDKInteraction',
        UTF8ToString(invocationId),
        UTF8ToString(methodName),
        UTF8ToString(reqJSON)
      );
    } catch (e) {
      console.warn('Failed to dispatch Web3Interaction event...', e);
    }
  }
});

    window.chatInterop = {
    hookClose: function (dotnetRef) {
    const modal = document.getElementById('chatModal');
    if (!modal) return;
    if (modal._listenerAttached) return;
    modal.addEventListener('hidden.bs.modal', function () {
    dotnetRef.invokeMethodAsync('StopChatPolling');
});
    modal._listenerAttached = true;
}
};

    window.scrollChatToBottom = function () {
    const el = document.querySelector('.chat-scroll-area');
    if (!el) return;
    el.scrollTop = el.scrollHeight;
};

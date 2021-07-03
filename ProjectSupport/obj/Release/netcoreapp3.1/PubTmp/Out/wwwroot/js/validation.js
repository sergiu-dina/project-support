function cbChange(obj) {
    if (obj.checked) {
        var cbs = document.getElementsByClassName("cb");
        for (var i = 0; i < cbs.length; i++)
            cbs[i].checked = false;
        obj.checked = true;
    }
}
//# sourceMappingURL=validation.js.map
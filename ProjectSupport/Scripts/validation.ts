function cbChange(obj: any): void {
    if (obj.checked) {
        const cbs = document.getElementsByClassName("cb") as HTMLCollectionOf<any>;
        for (let i: number = 0; i < cbs.length; i++)
            cbs[i].checked = false;
        obj.checked = true;
    }
}
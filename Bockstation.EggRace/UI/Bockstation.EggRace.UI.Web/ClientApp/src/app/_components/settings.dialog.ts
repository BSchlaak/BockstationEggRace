import { Component, Inject } from "@angular/core";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Settings } from "../_interfaces";

@Component({
    selector: 'settings-dialog',
    templateUrl: 'settings.dialog.html',
})
export class SettingsDialog {

    constructor(
        public dialogRef: MatDialogRef<SettingsDialog>,
        @Inject(MAT_DIALOG_DATA) public data: Settings) {}

    onCancel(): void {
        this.dialogRef.close();
    }

}

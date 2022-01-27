import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { faEgg, faListOl, faSlidersH, faUsers } from "@fortawesome/free-solid-svg-icons";
import { ResultService, TeamService } from './_services';
import * as fileSaver from 'file-saver';
import { MatDialog } from '@angular/material/dialog';
import { SettingsDialog } from './_components/settings.dialog';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  //#region Public fields
  public faEgg = faEgg;
  public faListOl = faListOl;
  public faSlidersH = faSlidersH;
  public faUsers = faUsers;
  public title = 'Das rollende Osterei';
  public isAdmin = false;
  //#endregion Public fields

  //#region Constructors
  constructor(
    private _dialog: MatDialog,
    private _location: Location,
    private _resultService: ResultService,
    private _teamService: TeamService,
  ) {
    this._location.onUrlChange((url, state) => {
      this.isAdmin = url.endsWith('/admin');
    });
  }
  //#endregion Constructors

  //#region Methods
  //#region Public
  public changeSettings() {
    this._dialog.open(SettingsDialog, {
      width: '640px',
      data: {},
    });
  }

  public downloadResults() {
    this._resultService.downloadResultsFile().subscribe((response: any) => {
      let blob: any = new Blob([response], { type: 'text/json; charset=utf-8' });
      const url = window.URL.createObjectURL(blob);
      fileSaver.saveAs(blob, 'Ergebnis.csv');
    });
  }
  //#endregion Public

  //#region Private
  private uploadTeams() {

  }
  //#endregion Private
  //#endregion Methods

  //#region Event handlers
  onFileSelected(event): void {
    const file: File = event.target.files[0];

    if (!!file) {
      this._teamService.uploadTeams(file).subscribe(foo => console.debug(foo));
    }
  }
  //#endregion Event handlers
}

import { Component, OnInit } from '@angular/core';

import { Result } from '../_interfaces';

import { ResultService } from '../_services';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent
  implements OnInit {

  //#region Public fields
  public displayedColumns: string[] = ['position', 'teamName', 'playerName', 'splitTime1', 'splitTime2', 'totalTime'];
  public results: Result[] = [];
  public sortedResults: Result[] = [];
  //#endregion Public fields

  constructor(
    private _resultService: ResultService,
  ) {

  }

  //#region Methods
  ngOnInit(): void {
    this._resultService.newResultReceived
      .subscribe(newResult => {
        let result = this.results.find(r => {
          return r.teamName === newResult.teamName && r.playerName === newResult.playerName;
        });

        if (!!result) {
          result.splitTime1 = newResult.splitTime1;
          result.splitTime2 = newResult.splitTime2;
          result.totalTime = newResult.totalTime;
        } else {
          this.results.push(newResult);
        }

        this.sortResults();
      });

    this._resultService.getResults()
      .subscribe(results => {
        this.results = results;
        this.sortResults();
      });
  }

  //#region Private
  private sortResults(): void {
    let sortedResults = JSON.parse(JSON.stringify(this.results));
    for (let result of sortedResults) {
      if (!!result.splitTime1) {
        result.splitTime1 = new Date(result.splitTime1);
      }
      if (!!result.splitTime2) {
        result.splitTime2 = new Date(result.splitTime2);
      }
      if (!!result.totalTime) {
        result.totalTime = new Date(result.totalTime);
      }
    }

    sortedResults.sort((a: Result, b: Result) => {
      if (!!a.totalTime && a.totalTime.getTime && !!b.totalTime && b.totalTime.getTime) {
        return a.totalTime.getTime() - b.totalTime.getTime();
      } else if (!!a.splitTime2 && a.splitTime2.getTime && !!b.splitTime2 && b.splitTime2.getTime) {
        return a.splitTime2.getTime() - b.splitTime2.getTime();
      } else if (!!a.splitTime1 && a.splitTime1.getTime && !!b.splitTime1 && b.splitTime1.getTime) {
        return a.splitTime1.getTime() - b.splitTime1.getTime();
      }
    });

    for (let i = 0; i < sortedResults.length; i++) {
      sortedResults[i].position = i + 1;
    }

    this.sortedResults = sortedResults;
  }
  //#endregion Private
  //#endregion Methods
}

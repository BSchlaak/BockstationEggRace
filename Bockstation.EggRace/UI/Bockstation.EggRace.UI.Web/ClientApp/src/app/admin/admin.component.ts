import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { faPlay } from "@fortawesome/free-solid-svg-icons";

import { Result, Team } from '../_interfaces';

import { ResultService, TeamService } from '../_services';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class AdminComponent
  implements OnInit {

  //#region Public fields
  public faPlay = faPlay;

  public displayedColumns: string[] = ['start', 'end', 'teamName', 'personName'];
  public teams: Team[] = [];
  public expandedTeam: Team | null;
  //#endregion Methods

  constructor(
    private _teamService: TeamService,
    private _resultService: ResultService,
  ) {
    // http.get<WeatherForecast[]>(baseUrl + 'weatherforecast').subscribe(result => {
    //   this.forecasts = result;
    // }, error => console.error(error));
  }

  //#region Methods
  ngOnInit(): void {
    this._resultService.newResultReceived
      .subscribe(newResult => {
        let team = this.teams.find(t => {
          return t.teamName === newResult.teamName;
        });

        if (!!team.results) {
          let result = team.results.find(r => {
            return r.playerName === newResult.playerName;
          });

          if (!!result) {
            result.splitTime1 = newResult.splitTime1;
            result.splitTime2 = newResult.splitTime2;
            result.totalTime = newResult.totalTime;
          }
        }
      });

    this._teamService.getTeams()
      .subscribe(teams => {
        this.teams = teams;
        for (let team of this.teams) {
          team.results = [];

          for (let i = 1; i <= team.eggs; i++) {
            team.results.push({ position: i, teamName: team.teamName, playerName: '' });
          }
        }

        this._resultService.getResults()
          .subscribe(results => {
            for (let result of results) {
              for (let team of this.teams) {
                if (result.teamName === team.teamName) {
                  for (let teamResult of team.results) {
                    if (result.playerName === teamResult.playerName || teamResult.playerName === '') {
                      teamResult.playerName = result.playerName;
                      teamResult.splitTime1 = result.splitTime1;
                      teamResult.splitTime2 = result.splitTime2;
                      teamResult.totalTime = result.totalTime;
                      break;
                    }
                  }
                }
              }
            }
          });
      });
  }

  startRace(result: Result): void {
    this._resultService.start(result);
  }
  //#endregion Methods
}

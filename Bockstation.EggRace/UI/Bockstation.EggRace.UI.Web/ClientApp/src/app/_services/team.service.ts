import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { Team } from '../_interfaces';

@Injectable({
  providedIn: 'root'
})
export class TeamService
  implements OnInit {

  //#region Private fields
  private _apiUrl: string;
  //#endregion Private fields

  constructor(
    private _http: HttpClient,
    @Inject('BASE_URL') baseUrl: string
  ) {
    this._apiUrl = baseUrl + 'api';
  }

  //#region Methods
  ngOnInit(): void {

  }

  //#region Public
  public getTeams(): Observable<Team[]> {
    const url = `${this._apiUrl}/Teams`;
    return this._http.get<any[]>(url)
      .pipe(
        map(data => {
          console.debug('Got data:', data);
          let teams: Team[] = data.map(t => {
            return {
              start: this.createDate(t.start.hours, t.start.minutes),
              end: this.createDate(t.end.hours, t.end.minutes),
              teamName: t.teamName,
              personName: t.personName,
              eggs: t.eggs,
            };
          });
          return teams;
        }),
      );
  }

  public uploadTeams(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('teams', file);
    
    const url = `${this._apiUrl}/Teams/file`;
    return this._http.post(url, formData);
  }
  //#endregion Public

  //#region Private
  private createDate(hours: number, minutes: number) {
    let date = new Date();
    date.setHours(hours);
    date.setMinutes(minutes);
    return date;
  }
  //#endregion Private
  //#endregion Methods
}


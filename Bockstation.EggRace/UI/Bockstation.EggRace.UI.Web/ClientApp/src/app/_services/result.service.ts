import { HttpClient } from '@angular/common/http';
import { EventEmitter, Inject, Injectable, Output } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';

import { Result } from '../_interfaces';

@Injectable({
  providedIn: 'root'
})
export class ResultService {

  //#region Events
  @Output() newResultReceived = new EventEmitter<Result>();
  //#endregion Events

  //#region Private fields
  private _apiUrl: string;
  private _hubUrl: string;
  private _results: Result[] = [];
  private _hubConnection: signalR.HubConnection;
  //#endregion Private fields

  constructor(
    private _http: HttpClient,
    @Inject('BASE_URL') baseUrl: string
  ) {
    this._apiUrl = baseUrl + 'api';
    this._hubUrl = baseUrl + 'hubs';
    this.connectToHub();
  }

  //#region Methods
  //#region Public
  public start(result: Result): void {
    const url = `${this._apiUrl}/Results/start`;
    this._http.post(url, result)
      .subscribe({
        next: data => {
          console.debug('Done!', data);
        },
        complete: () => {
          console.debug('Completed!');
        },
        error: error => {
          console.error('An error occurred:', error);
        },
      });
    //this.simulate(result);
  }

  public getResults(): Observable<Result[]> {
    const url = `${this._apiUrl}/Results`;
    return this._http.get<any[]>(url)
      .pipe(
        map(data => {
          console.debug('Got data:', data);
          this._results = data.map(r => {
            return {
              position: r.position,
              teamName: r.teamName,
              playerName: r.playerName,
              splitTime1: this.createDate(r.splitTime1),
              splitTime2: this.createDate(r.splitTime2),
              totalTime: this.createDate(r.totalTime),
            };
          });
          return this._results;
        }),
      );
  }

  public downloadResultsFile(): any {
    const url = `${this._apiUrl}/Results/file`;
    return this._http.get(url, { responseType: 'text' });
  }
  //#endregion Public

  //#region Private
  private connectToHub() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this._hubUrl}/Results`)
      .build();

      this._hubConnection
        .start()
        .then(() => console.log('Connection started'))
        .catch(err => console.log('Error while starting connection: ' + err))

      this._hubConnection.on('ReceiveResult', data => {
        console.debug('Received result', data);
        const result = {
          position: data.position,
          teamName: data.teamName,
          playerName: data.playerName,
          splitTime1: this.createDate(data.splitTime1),
          splitTime2: this.createDate(data.splitTime2),
          totalTime: this.createDate(data.totalTime),
        };

        this.newResultReceived.emit(result);
      });
    }

  // private simulate(result: Result) {
  //   setTimeout(() => {
  //     console.debug('Start emitting results ...');
  //     this.emitNewResult(JSON.parse(JSON.stringify(result)));
  //   }, Math.random() * 1000);
  // }

  // private emitNewResult(result: Result): void {
  //   const startTime = new Date();

  //   setTimeout(() => {
  //     result.splitTime1 = new Date(new Date().getTime() - startTime.getTime());
  //     this.newResultReceived.emit(result);

  //     setTimeout(() => {
  //       result.splitTime2 = new Date(new Date().getTime() - startTime.getTime());
  //       this.newResultReceived.emit(result);

  //       setTimeout(() => {
  //         result.totalTime = new Date(new Date().getTime() - startTime.getTime());
  //         this.newResultReceived.emit(result);
  //       }, Math.random() * 2000);
  //     }, Math.random() * 2000);
  //   }, Math.random() * 2000);
  // }

  private createDate(time: any) {
    if (!!time && time.hasValue) {
      let date = new Date();
      date.setHours(time.value.hours);
      date.setMinutes(time.value.minutes);
      date.setSeconds(time.value.seconds);
      date.setMilliseconds(time.value.milliseconds);
      return date;
    }

    return null;
  }
  // private createDate(hours: number, minutes: number) {
  //   let date = new Date();
  //   date.setHours(hours);
  //   date.setMinutes(minutes);
  //   return date;
  // }
  //#endregion Private
  //#endregion Methods
}

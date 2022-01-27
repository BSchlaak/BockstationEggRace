import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTableModule } from '@angular/material/table';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { AdminComponent } from './admin/admin.component';
import { SettingsDialog } from './_components/settings.dialog';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AdminComponent,
    SettingsDialog,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'admin', component: AdminComponent },
    ]),
    BrowserAnimationsModule,

    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatInputModule,
    MatSlideToggleModule,
    MatTableModule,
    MatToolbarModule,
    FontAwesomeModule,
  ],
  exports: [
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatInputModule,
    MatSlideToggleModule,
    MatTableModule,
    MatToolbarModule,
  ],
  providers: [],
  entryComponents: [
    SettingsDialog,
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }

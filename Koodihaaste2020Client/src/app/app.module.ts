import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { DragDropModule } from '@angular/cdk/drag-drop';


import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';



// Components
import { AppComponent } from './app.component';
import { CityMap } from './components/city-map/city-map.component';
import { GuidanceBox } from './components/guidance-box/guidance-box.component';

// Providers
import { RouteCalculator } from './services/routeCalculator'
import { BusDataFetcher } from './services/busDataFetcher';

import { HttpClientModule } from '@angular/common/http';

import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {MatSnackBarModule} from '@angular/material/snack-bar';


@NgModule({
  declarations: [
    AppComponent,
    CityMap,
    GuidanceBox
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    DragDropModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MatSnackBarModule
  ],
  providers: [
    RouteCalculator,
    BusDataFetcher
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

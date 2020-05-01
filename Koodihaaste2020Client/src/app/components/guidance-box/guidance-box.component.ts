import { Component, OnInit } from '@angular/core';
import { BusDataFetcher } from '../../services/busDataFetcher';
import { RouteCalculator } from '../../services/routeCalculator';
import { BusStop } from '../../models/busStop';
import { BusLineRoute } from 'src/app/models/busLineRoute';

/**
 * This component handles the guidance box logic.
 * It also handles calculating and displaying the optimal route.
 */
@Component({
  selector: 'guidance-box',
  templateUrl: './guidance-box.component.html',
  styleUrls: ['./guidance-box.component.scss'],
  providers: [BusDataFetcher]
})
export class GuidanceBox implements OnInit {
  constructor(
    private busDataFetcher: BusDataFetcher,
    private routeCalculator: RouteCalculator,
    ) {
  }

  ngOnInit() {
    this.busDataFetcher.getAllBusStops().subscribe((stops: BusStop[]) => {
      this.busStops = stops;
    });
  }

  calculatingRoute: boolean;
  busStops: BusStop[];
  stopErrorMessage: string = '';
  routeList: BusLineRoute[] = [];
  routeTotalDuration: number;

  stopData = {
    fromStop: '',
    fromStopValidated: false,
    toStop: '',
    toStopValidated: false,
  }
  
  showText: boolean = true;

  // Toggles if the text in the box is shown
  toggleShowText() {
    this.showText = !this.showText;
  }

  // Validates the typed stop. Both fromStopValidated and toStopValidated need to be true to be able to calcluate the route.
  validateStop(stopType: string, stopName: string) {
    // First check if written stop name is empty
    if(stopName === '') {
      if(stopType === 'fromStop') {
        this.stopData.fromStopValidated = false;
      } else {
        this.stopData.toStopValidated = false;
      }
      this.stopErrorMessage = '';
      return;
    }

    let upperCaseStopName = stopName.toUpperCase();

    // Then check if the bus stop with that name exists.
    let exists = false;
    this.busStops.forEach(stop => {
      if(stop.Name.toUpperCase() === upperCaseStopName) {
        exists = true;
      }
    });

    if(!exists) { // If doesn't exist, invalidate the input 
      if(stopType === 'fromStop') {
        this.stopData.fromStopValidated = false;
        this.stopData.fromStop = '';
        this.stopErrorMessage = 'Syöttämääsi lähtöpysäkkiä ei ole olemassa.';
      } else {
        this.stopData.toStopValidated = false;
        this.stopData.toStop = '';
        this.stopErrorMessage = 'Syöttämääsi päätepysäkkiä ei ole olemassa.';
      }
    } else { // If exists, mark input as validated
      this.stopErrorMessage = '';
      if(stopType === 'fromStop') {
        this.stopData.fromStopValidated = true;
      } else {
        this.stopData.toStopValidated = true;
      }
    }
  }

  // Send api request to calculate the shortest route
  calculateRoute() {
    // Handle case that both stops are the same
    if (this.stopData.fromStop.toUpperCase() === this.stopData.toStop.toUpperCase()) {
      this.stopErrorMessage = 'Pysäkit eivät saa olla samat.';
      return;
    } else {
      this.stopErrorMessage = '';
    }

    if(this.stopData.fromStop.length > 0 && this.stopData.toStop.length > 0) {
      this.calculatingRoute = true;
      this.routeCalculator.getShortestRoute(this.stopData.fromStop, this.stopData.toStop).subscribe((routes: BusLineRoute[]) => {
        this.routeList = routes;
        let sum = 0;
        routes.forEach(route => {
          sum += route.Route.Duration;
        })
        this.routeTotalDuration = sum;
        this.stopErrorMessage = '';
        this.calculatingRoute = false;
      }, err => { 
        this.stopErrorMessage = 'Laskennassa tapahtui virhe. Kokeile hetken päästä uudelleen.';
        this.calculatingRoute = false;
       }
      );
    }
  }
}

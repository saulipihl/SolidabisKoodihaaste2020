import { Component } from '@angular/core';
import { RouteCalculator } from './services/routeCalculator';

import { Observable, throwError } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [RouteCalculator]
})
export class AppComponent {
  constructor(private routeCalculator: RouteCalculator) {
    //routeCalculator.getShortestRoute('A', 'P').subscribe(x => console.log(x));
  }
  title = 'Koodihaaste2020Client';

}

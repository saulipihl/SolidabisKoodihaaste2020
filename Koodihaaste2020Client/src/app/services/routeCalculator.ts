import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class RouteCalculator {
    constructor(private http: HttpClient) {}

    getShortestRoute(fromStopName: string, toStopName: string) {
        return this.http.get(`http://localhost:57966/api/busdata/calculateRoute?fromStopName=${fromStopName}&toStopName=${toStopName}`);
    }
}
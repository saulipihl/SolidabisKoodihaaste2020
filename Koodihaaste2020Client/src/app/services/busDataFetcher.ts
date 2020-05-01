import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';



@Injectable()
export class BusDataFetcher {
    apiUrl: string = environment.apiUrl;
    constructor(private http: HttpClient) {}

    getAllBusStops() {
        return this.http.get(`${environment.apiUrl}busdata/getbusstops`);
    }
}
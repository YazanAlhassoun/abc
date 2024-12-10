import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "projects/admin/src/environments/environment";
import { StatisticsCount, RevenueChart } from "../interface/dashboard.interface";

@Injectable({
  providedIn: "root",
})
export class DashboardService {

  constructor(private http: HttpClient) {}

  getStatisticsCount(): Observable<StatisticsCount> {
    return this.http.get<StatisticsCount>(`${environment.URL}/count.json`);
  }

  getRevenueChart(): Observable<RevenueChart> {
    return this.http.get<RevenueChart>(`${environment.URL}/chart.json`);
  }

}

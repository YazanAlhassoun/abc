import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

import { AccountUser } from "../interface/account.interface";
import { environment } from "projects/user/src/environments/environment";

@Injectable({
  providedIn: "root",
})
export class AccountService {

  constructor(private http: HttpClient) {}

  getUserDetails(): Observable<AccountUser> {
    return this.http.get<AccountUser>(`${environment.URL}/account.json`);
  }

}

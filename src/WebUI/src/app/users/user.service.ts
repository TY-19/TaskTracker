import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { UserProfile } from "../models/user-profile";
import { RegistrationRequest } from "../models/registration-request";
import { RegistrationResult } from "../models/registration-result";

@Injectable({
    providedIn: 'root',
  })
export class UserService {
    
  constructor(private http: HttpClient) {

  }

  getUsers() : Observable<UserProfile[]> {
    const url = "/api/users";
    return this.http.get<UserProfile[]>(url);
  }

  getUser(userName: string) : Observable<UserProfile> {
    const url = "/api/users/" + userName;
    return this.http.get<UserProfile>(url);
  }

  addUser(user: RegistrationRequest): Observable<RegistrationResult> {
    const url = "/api/users";
    return this.http.post<RegistrationResult>(url, user);
  }

}
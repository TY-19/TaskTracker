import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { UserProfile } from "../account/user-profile";

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

}
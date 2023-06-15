import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { UserProfile } from "./user-profile";

@Injectable({
    providedIn: 'root',
  })
export class AccountService {
    
  constructor(private http: HttpClient) {
 
  }

  getUserProfile() : Observable<UserProfile> {
    const url = "api/account/profile";
    return this.http.get<UserProfile>(url);
  }

  updateUserProfile(profile: any) {
    const url = "api/account/profile";
    return this.http.put<UserProfile>(url, profile);
  }
}
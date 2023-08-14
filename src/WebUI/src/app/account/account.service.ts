import { HttpClient, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { UserProfile } from "../models/user-profile";
import { UserUpdateModel } from "../models/update-models/user-update-model";
import { ChangePasswordModel } from "../models/update-models/change-password.model";

@Injectable({
    providedIn: 'root',
  })
export class AccountService {
    
  constructor(private http: HttpClient) {
 
  }

  getUserProfile(): Observable<UserProfile> {
    const url = "api/account/profile";
    return this.http.get<UserProfile>(url);
  }

  updateUserProfile(profile: UserUpdateModel): Observable<HttpResponse<Object>> {
    const url = "api/account/profile";
    return this.http.put(url, profile, { observe: 'response'});
  }

  changePassword(passwordModel: ChangePasswordModel): Observable<HttpResponse<Object>> {
    const url = "api/account/profile/changepassword";
    return this.http.put(url, passwordModel, { observe: 'response'});
  }
}
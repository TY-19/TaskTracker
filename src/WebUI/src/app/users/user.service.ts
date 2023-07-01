import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { UserProfile } from "../models/user-profile";
import { RegistrationRequest } from "../models/registration-request";
import { RegistrationResult } from "../models/registration-result";
import { UserUpdateModel } from "../models/update-models/user-update-model";

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

  updateUser(userName: string, updateModel: UserUpdateModel) {
    const url = "/api/users/" + userName;
    return this.http.put(url, updateModel);
  }

  changePassword(userName: string, newPassword: string) {
    const url = "/api/users/" + userName + "/changepassword";
    return this.http.put(url, { newPassword: newPassword });
  }

  deleteUser(userName: string) {
    const url = "/api/users/" + userName;
    return this.http.delete(url);
  }

}
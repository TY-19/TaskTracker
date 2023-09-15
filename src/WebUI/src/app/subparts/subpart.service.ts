import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Subpart } from "../models/subpart";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root',
  })
export class SubpartService {
    
  constructor(private http: HttpClient) {

  }

  getSubparts(boardId: number | string, taskId: number | string): Observable<Subpart[]> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/tasks/" + taskId + "/subparts";
    return this.http.get<Subpart[]>(url);
  }

  updateSubpart(boardId: number | string, taskId: number | string, subpartId: number | string,
    subpart: Subpart): Observable<Object> {
      const url = environment.baseUrl + "api/boards/" + boardId + "/tasks/" + taskId
        + "/subparts/" + subpartId;
      return this.http.put(url, subpart);
  }
}
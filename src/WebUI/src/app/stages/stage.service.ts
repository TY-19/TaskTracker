import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Stage } from "../models/stage";

@Injectable({
    providedIn: 'root',
  })
export class StageService {
    
  constructor(private http: HttpClient) {

  }

  getStages(boardId: string) : Observable<Stage[]> {
    const url = "/api/boards/" + boardId + "/stages";
    return this.http.get<Stage[]>(url);
  }

  moveStage(boardId: string, stageId: string, forward: boolean) {
    let url = "/api/boards/" + boardId + "/stages/" + stageId;
    if (forward) url += "/moveforward";
    else url += "/moveback";
    return this.http.put(url, {});
  }
}
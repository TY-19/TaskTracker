import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Stage } from "../models/stage";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root',
  })
export class StageService {
    
  constructor(private http: HttpClient) {

  }

  getStages(boardId: number | string) : Observable<Stage[]> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/stages";
    return this.http.get<Stage[]>(url);
  }

  getStage(boardId: number | string, stageId: number | string) : Observable<Stage> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/stages/" + stageId;
    return this.http.get<Stage>(url);
  }

  moveStage(boardId: string, stageId: string, forward: boolean) {
    let url = environment.baseUrl + "api/boards/" + boardId + "/stages/" + stageId;
    if (forward) url += "/moveforward";
    else url += "/moveback";
    return this.http.put(url, {});
  }

  createStage(boardId: string, stage: Stage) {
    const url = environment.baseUrl + "api/boards/" + boardId + "/stages/";
    return this.http.post(url, stage);
  }

  updateStage(boardId: string, stage: Stage) {
    const url = environment.baseUrl + "api/boards/" + boardId + "/stages/" + stage.id;
    return this.http.put(url, stage);
  }

  deleteStage(boardId: string, stageId: string) {
    const url = environment.baseUrl + "api/boards/" + boardId + "/stages/" + stageId;
    return this.http.delete(url);
  }
}
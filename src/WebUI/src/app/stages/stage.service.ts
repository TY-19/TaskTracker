import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Stage } from "../models/stage";
import { environment } from "src/environments/environment";
import { StageUpdateModel } from "../models/update-models/stage-update-model";

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

  moveStage(boardId: number | string, stageId: number | string, forward: boolean)
    : Observable<Object> {
      let url = environment.baseUrl + "api/boards/" + boardId + "/stages/" + stageId;
      if (forward) url += "/moveforward";
      else url += "/moveback";
      return this.http.put(url, {});
  }

  createStage(boardId: number | string, stage: StageUpdateModel): Observable<Object> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/stages/";
    return this.http.post(url, stage);
  }

  updateStage(boardId: number | string, stageId: number | string, stage: StageUpdateModel)
    : Observable<Object> {
      const url = environment.baseUrl + "api/boards/" + boardId + "/stages/" + stageId;
      return this.http.put(url, stage);
  }

  deleteStage(boardId: number | string, stageId: string): Observable<Object> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/stages/" + stageId;
    return this.http.delete(url);
  }
}
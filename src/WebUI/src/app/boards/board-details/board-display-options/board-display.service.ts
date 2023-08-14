import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { sortTasksByDeadlineAsc, sortTasksByDeadlineDesc, sortTasksByNameAsc, sortTasksByNameDesc } from "src/app/common/helpers/comparators";
import { filterAllIncompletedTasks, filterAllMyTasks, filterAllTasks, filterMyIncompletedTasks } from "src/app/common/helpers/filters";
import { AssignmentComparator } from "src/app/models/custom-types/assignment-comparator";
import { AssignmentFilter } from "src/app/models/custom-types/assignment-filter";

@Injectable({
    providedIn: 'root',
  })
export class BoardDisplayService {
    
    private _sortFieldKey: string = "sortField";
    private _sortOrderKey: string = "sortOrder";

    private _sortingFunction = new Subject<AssignmentComparator>();
    public sortingFunction = this._sortingFunction.asObservable();

    private _showOnlyMyTasksKey: string = "onlyMyTasks";
    private _showCompletedTasksKey: string = "completedTasks";

    private _filterFunction = new Subject<AssignmentFilter>();
    public filterFunction = this._filterFunction.asObservable();

    getShowOnlyMyTasks(): boolean {
        return localStorage.getItem(this._showOnlyMyTasksKey) === "true";
    }

    getShowCompletedTasks(): boolean {
        return localStorage.getItem(this._showCompletedTasksKey) !== "false";
    }

    setShowOnlyMyTasks(showOnlyMyTasks: boolean): void {
        localStorage.setItem(this._showOnlyMyTasksKey, showOnlyMyTasks.toString());
    }

    setShowCompletedTasks(showCompletedTasks: boolean): void {
        localStorage.setItem(this._showCompletedTasksKey, showCompletedTasks.toString());
    }

    doFiltration(): void {
        let showOnlyMyTasks = this.getShowOnlyMyTasks();
        let showCompletedTasks = this.getShowCompletedTasks();
        switch(true) {
            case showOnlyMyTasks && showCompletedTasks:
                this._filterFunction.next(filterAllMyTasks);
                break;
            case showOnlyMyTasks && !showCompletedTasks:
                this._filterFunction.next(filterMyIncompletedTasks);
                break;
            case !showOnlyMyTasks && showCompletedTasks:
                this._filterFunction.next(filterAllTasks);
                break;
            case !showOnlyMyTasks && !showCompletedTasks:
                this._filterFunction.next(filterAllIncompletedTasks);
                break;
            default:
                this._filterFunction.next(filterAllTasks);
                break;
        }
    }

    getSortField(): string {
        return localStorage.getItem(this._sortFieldKey) ?? "name";
    }

    setSortField(field: string): void {
        localStorage.setItem(this._sortFieldKey, field);
    }

    getSortOrder(): string {
        return localStorage.getItem(this._sortOrderKey) ?? "asc";
    }

    setSortOrder(order: string): void {
        localStorage.setItem(this._sortOrderKey, order);
    }

    doSorting(): void {
        let field = this.getSortField();
        let order = this.getSortOrder();
        switch (true) {
            case field === 'name' && order === 'asc': 
                this._sortingFunction.next(sortTasksByNameAsc);
                break;
            case field === 'name' && order === 'desc': 
                this._sortingFunction.next(sortTasksByNameDesc);
                break;
            case field === 'deadline' && order === 'asc':
                this._sortingFunction.next(sortTasksByDeadlineAsc);
                break;
            case field === 'deadline' && order === 'desc':
                this._sortingFunction.next(sortTasksByDeadlineDesc);
                break;
            default:
                this._sortingFunction.next(sortTasksByNameAsc);
                break;
        }
    }
}

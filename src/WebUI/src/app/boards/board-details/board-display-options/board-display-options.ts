import { AssignmentComparator } from "src/app/models/custom-types/assignment-comparator";
import { BoardDisplayService } from "./board-display.service";
import { AssignmentFilter } from "src/app/models/custom-types/assignment-filter";

export class BoardDisplayOptions {
  
  constructor(private boardDisplayService: BoardDisplayService) {
    
  }

  private customSortingFunction?: AssignmentComparator;
  private customFilterFunction?: AssignmentFilter;

  private defaultSortingFunction: AssignmentComparator = 
    (a, b) => a.topic.toLowerCase() > b.topic.toLowerCase() ? 1 : -1;
  private defaultFilterFunction: AssignmentFilter = (a) => true;

  get sortingFunction(): AssignmentComparator {
    return this.customSortingFunction ?? this.defaultSortingFunction;
  }
  get filterFunction(): AssignmentFilter {
    return this.customFilterFunction ?? this.defaultFilterFunction;
  }

  applyDisplayOptions() {
    this.setDisplayOptions();
    this.boardDisplayService.doSorting();
    this.boardDisplayService.doFiltration();
  }

  setDisplayOptions() {
    this.boardDisplayService.sortingFunction
      .subscribe(func => {
        this.customSortingFunction = func;
      });
    this.boardDisplayService.filterFunction
      .subscribe(filter => {
        this.customFilterFunction = filter;
      });
  }
}

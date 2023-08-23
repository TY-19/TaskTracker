import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";

export class MatTableHelper<T extends Record<string, any>> {
    
    private isSortInitiated: boolean = false;
    private isPaginationInitiated: boolean = false;

    constructor(private SpecificSortingDataAccessor?: 
      (data: T, sortHeaderId: string) => string | number) {

    }

    initiateTable(dataSource?: MatTableDataSource<T>, sort?: MatSort,
      paginator?: MatPaginator): void {
        if (dataSource) {
            this.initiateSort(dataSource, sort);
            this.initiatePagination(dataSource, paginator);
        }
    }

    private initiatePagination(dataSource?: MatTableDataSource<T>,
      paginator?: MatPaginator): void {
        if (!this.isPaginationInitiated && paginator) {
          dataSource!.paginator = paginator;
          this.isPaginationInitiated = true;
        }
      }
    
    private initiateSort(dataSource?: MatTableDataSource<T>, sort?: MatSort): void {
        if (!this.isSortInitiated && sort) {
            sort.disableClear = true;
            dataSource!.sort = sort;
            dataSource!.sortingDataAccessor = this.SpecificSortingDataAccessor 
              ?? this.GeneralSortingDataAccessor;
            this.isSortInitiated = true;
        }
      }

    private GeneralSortingDataAccessor(data: T, sortHeaderId: string): string | number {
      if (typeof(data[sortHeaderId]) === 'string')
        return data[sortHeaderId].toLowerCase()
      else
        return data[sortHeaderId];
    }
}
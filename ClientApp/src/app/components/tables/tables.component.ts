import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BookModel } from 'src/app/models/BookModel';
import { BookReaderModel } from 'src/app/models/BookReaderModel';
import { LibraryModel } from 'src/app/models/LibraryModel';
import { ReaderModel } from 'src/app/models/ReaderModel';
import { TablePattern } from 'src/app/models/TablePattern';
import { TableApiService } from 'src/app/services/base-table-api.service';

@Component({
  selector: 'app-tables',
  templateUrl: './tables.component.html',
  styleUrls: ['./tables.component.css']
})
export class TablesComponent {
  private _tablePatterns: { pattern: TablePattern, selected: boolean }[] = [
    {
      selected: true,
      pattern: new TablePattern('Library', new TableApiService('library', this._http), LibraryModel)
    },
    {
      selected: false,
      pattern: new TablePattern('Book', new TableApiService('book', this._http), BookModel)
    },
    {
      selected: false,
      pattern: new TablePattern('Reader', new TableApiService('reader', this._http), ReaderModel)
    },
    {
      selected: false,
      pattern: new TablePattern('BookReader', new TableApiService('bookreader', this._http), BookReaderModel)
    }
  ];

  public get tablePattern(): TablePattern {
    if (!this._tablePatterns.some(table => table.selected)) {
      this._tablePatterns[0].selected = true;
    }
    var selectedTable = this._tablePatterns.find(table => table.selected)?.pattern ?? this._tablePatterns[0].pattern;
    return selectedTable;
  }

  public get tableHeaders(): { name: string, selected: boolean }[] {
    return this._tablePatterns.map(table => {
      return {
        name: table.pattern.name,
        selected: table.selected
      };
    });
  }

  constructor(private _http: HttpClient, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.selectTable(params['name']);
    });
  }

  selectTable(name: string): void {
    this._tablePatterns.forEach(table => {
      table.selected = table.pattern.name === name;
    });
  }
}

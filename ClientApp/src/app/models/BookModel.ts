import { LibraryModel } from "./LibraryModel";

export class BookModel {
    public id: number = 0;
    public name: string = '';
    public author: string = '';
    public year: number = 0;
    public libraryId: number = 0;
    public library: LibraryModel = new LibraryModel();
}
export type OmitId<T> = Omit<T, 'id'> // Useful for POST requests where you don't need to send the id.

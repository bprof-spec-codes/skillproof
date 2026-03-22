export interface JwtPayload {
    subject?: string
    roles?: string[] | string
    expiration?: number
}

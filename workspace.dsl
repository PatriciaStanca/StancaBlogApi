workspace "StancaBlogApi" "C4 model for the StancaBlogApi project" {

    !identifiers hierarchical

    model {
        writer = person "Blog Writer" "Registers an account, logs in, creates posts, and writes comments."
        reader = person "Reader" "Reads posts and comments without logging in."

        stancaBlog = softwareSystem "StancaBlogApi" "ASP.NET Core Web API for a blog community." {
            api = container "Web API" "Exposes REST endpoints for auth, blog posts, categories, and comments." "ASP.NET Core 8" {
                tags "API"
            }

            db = container "Database" "Stores users, blog posts, categories, and comments." "SQL Server" {
                tags "Database"
            }

            jwt = container "JWT Token Generator" "Generates signed JWT tokens during login." "Custom .NET component" {
                tags "Security"
            }
        }

        swagger = softwareSystem "Swagger UI" "Interactive API documentation for developers."
        postman = softwareSystem "Postman" "API client for manual testing."

        writer -> stancaBlog.api "Uses (register/login, CRUD for posts/comments)" "HTTPS/JSON"
        reader -> stancaBlog.api "Reads public posts, categories, and comments" "HTTPS/JSON"

        swagger -> stancaBlog.api "Calls API endpoints" "HTTPS/JSON"
        postman -> stancaBlog.api "Calls API endpoints" "HTTPS/JSON"

        stancaBlog.api -> stancaBlog.jwt "Requests token during login" "In-process call"
        stancaBlog.api -> stancaBlog.db "Reads/writes data via EF Core" "SQL/TDS"
    }

    views {
        systemContext stancaBlog "SystemContext" {
            include *
            autolayout lr
        }

        container stancaBlog "Containers" {
            include *
            autolayout lr
        }

        styles {
            element "Element" {
                color #0b4f6c
                stroke #0b4f6c
                strokeWidth 3
                shape roundedbox
                background #e9f3f8
            }

            element "Person" {
                shape person
                background #fdf6e3
                color #3b2f2f
                stroke #b58900
            }

            element "Software System" {
                background #d9eaf4
            }

            element "API" {
                background #cfe8ff
                shape roundedbox
            }

            element "Security" {
                background #fde2e4
            }

            element "Database" {
                shape cylinder
                background #d8f3dc
            }

            relationship "Relationship" {
                thickness 3
                color #1d3557
            }
        }
    }

    configuration {
        scope softwaresystem
    }
}

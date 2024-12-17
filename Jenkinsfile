pipeline {
    agent any

    environment {
        DOCKER_COMPOSE_PATH = 'docker-compose.yml'
    }

    stages {
        stage('Checkout Code') {
            steps {
                echo 'Checking out source code...'
                checkout scm
            }
        }

        stage('Deploy with Docker Compose') {
            steps {
                echo 'Deploying services using Docker Compose...'
                bat 'docker-compose -f Jenkins/.jenkins/workspace/MovieTheater/MovieTheaterAPI/docker-compose.yml down' // Dừng các container cũ
                bat 'docker-compose -f Jenkins/.jenkins/workspace/MovieTheater/MovieTheaterAPI/docker-compose.yml up --build -d' // Build lại image và khởi động container
            }
        }

        stage('Verify Deployment') {
            steps {
                echo 'Verifying deployment...'
                bat 'docker ps'
            }
        }
    }

    post {
        success {
            echo 'Deployment completed successfully!'
        }
        failure {
            echo 'Deployment failed. Check logs for details.'
        }
    }
}
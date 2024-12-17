pipeline {
    agent  any
    stages {
        stage('Test Docker') {
            steps {
                script {
                    sh 'docker --version'
                }
            }
        }
        stage('Clone Repository') {
    steps {
        script {
            try {
                echo "Cloning the repository..."
                git branch: 'main', url: 'https://github.com/Tlordisme/Cinema_ManageMent.git'
            } catch (Exception e) {
                echo "Failed to clone repository: ${e.message}"
                currentBuild.result = 'FAILURE'
                error "Stopping pipeline because the repository could not be cloned."
            }
        }
    }
}

        stage('Build Docker Image') {
            steps {
                script {
                    try {
                        echo "Building Docker image..."
                        sh "docker build -t demo:v1 ." 
                    } catch (Exception e) {
                        echo "Failed to build Docker image: ${e.message}"
                        currentBuild.result = 'FAILURE'
                        error "Stopping pipeline because the Docker image build failed."
                    }
                }
            }
        }

        
    }
   
}
package uk.ac.kingston.ci7250.ims.service;

import org.springframework.stereotype.Service;
import uk.ac.kingston.ci7250.ims.domain.Patient;
import uk.ac.kingston.ci7250.ims.domain.WorkflowTask;
import uk.ac.kingston.ci7250.ims.repo.PatientRepository;
import uk.ac.kingston.ci7250.ims.repo.WorkflowTaskRepository;

import java.math.BigDecimal;
import java.time.Instant;
import java.util.List;

@Service
public class WorkflowService {
    private final PatientRepository patientRepository;
    private final WorkflowTaskRepository taskRepository;

    public WorkflowService(PatientRepository patientRepository, WorkflowTaskRepository taskRepository) {
        this.patientRepository = patientRepository;
        this.taskRepository = taskRepository;
    }

    public WorkflowTask addTask(long patientId, String type, BigDecimal cost, String performedBy) {
        if (type == null || type.isBlank()) {
            throw new IllegalArgumentException("Task type is required");
        }
        if (cost == null || cost.signum() < 0) {
            throw new IllegalArgumentException("Task cost must be >= 0");
        }

        Patient p = patientRepository.require(patientId);
        long id = taskRepository.nextId();
        WorkflowTask task = new WorkflowTask(id, patientId, type, Instant.now(), cost, performedBy);
        taskRepository.create(task);
        p.getTaskIds().add(task.getId());
        return task;
    }

    public List<WorkflowTask> listTasksForPatient(long patientId) {
        patientRepository.require(patientId);
        return taskRepository.findByPatientIdSorted(patientId);
    }

    public void deleteTask(long taskId) {
        taskRepository.delete(taskId);
    }
}

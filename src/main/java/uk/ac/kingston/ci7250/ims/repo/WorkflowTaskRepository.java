package uk.ac.kingston.ci7250.ims.repo;

import org.springframework.stereotype.Repository;
import uk.ac.kingston.ci7250.ims.domain.WorkflowTask;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.NoSuchElementException;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicLong;

@Repository
public class WorkflowTaskRepository {
    private final AtomicLong idSeq = new AtomicLong(3000);
    private final Map<Long, WorkflowTask> tasks = new ConcurrentHashMap<>();

    public WorkflowTask create(WorkflowTask task) {
        tasks.put(task.getId(), task);
        return task;
    }

    public long nextId() {
        return idSeq.incrementAndGet();
    }

    public Optional<WorkflowTask> findById(long id) {
        return Optional.ofNullable(tasks.get(id));
    }

    public WorkflowTask require(long id) {
        return findById(id).orElseThrow(() -> new NoSuchElementException("Task not found: " + id));
    }

    public List<WorkflowTask> findByPatientIdSorted(long patientId) {
        List<WorkflowTask> list = new ArrayList<>();
        for (WorkflowTask t : tasks.values()) {
            if (t.getPatientId() == patientId) {
                list.add(t);
            }
        }
        list.sort(Comparator.comparing(WorkflowTask::getTimestamp));
        return list;
    }

    public void delete(long id) {
        if (tasks.remove(id) == null) {
            throw new NoSuchElementException("Task not found: " + id);
        }
    }
}
